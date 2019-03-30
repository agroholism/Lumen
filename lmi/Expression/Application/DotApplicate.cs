using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Light {
    internal class DotApplicate : Expression {
        internal DotExpression callable;
        internal List<Expression> argumentsExperssions;

        public DotApplicate(Expression callable, List<Expression> exps) {
            this.callable = callable as DotExpression;
            this.argumentsExperssions = exps;
        }

        public Value Eval(Scope e) {
            Value obj = this.callable.expression.Eval(e);

            Value function;

            String name = this.callable.nameVariable;

            if (obj is Module module) {
                if (!module.Contains(name)) {
                    throw new LumenException(Exceptions.MODULE_DOES_NOT_CONTAINS_FUNCTION.F(name, module.name));
                }

                function = module.GetField(name, e);

                return new Applicate(new ValueE(function), this.argumentsExperssions, -1, "").Eval(e);
            }

            if (obj is IObject iob) {
                Value v = iob.GetField(name, e);

                if (v is Fun fn) {
                    Scope innerScope = new Scope(e) { ["self"] = fn };
                    List<Value> argse = new List<Value>();
                    argse.AddRange(this.EvalArguments(e));
                    argse.Add(obj);

                    return fn.Run(innerScope, argse.ToArray());
                }

                throw new LumenException(Exceptions.NOT_A_FUNCTION.F(this.callable));
            }

            IObject cls = obj.Type;

            if (!cls.TryGetField(name, out Value prf)) {
                try {
                    return new Applicate(this.callable, this.argumentsExperssions, -1, "").Eval(e);
                } catch {
                    throw new LumenException(Exceptions.MODULE_DOES_NOT_CONTAINS_FUNCTION.F(name, cls));
                }
            }

            Fun functio = cls.GetField(name, e) as Fun;

            Scope innerScop = new Scope(e) { ["self"] = functio };

            List<Value> Objects = new List<Value> { };

            Objects.AddRange(this.EvalArguments(e));
            Objects.Add(obj);
            return functio.Run(innerScop, Objects.ToArray());
        }

        private List<Value> EvalArguments(Scope scope) {
            List<Value> arguments = new List<Value>();

            foreach (Expression i in this.argumentsExperssions) {
                if (i is SpreadE) {
                    foreach (Value j in Converter.ToList(i.Eval(scope), scope)) {
                        arguments.Add(j);
                    }
                } else {
                    arguments.Add(i.Eval(scope));
                }
            }

            return arguments;
        }

        public Expression Closure(List<String> visible, Scope thread) {
            return new DotApplicate(this.callable.Closure(visible, thread), this.argumentsExperssions.Select(i => i.Closure(visible, thread)).ToList());
        }


        public override String ToString() {
            if (this.callable is DotExpression de && de.nameVariable == "[]") {
                return de.expression + "[" + String.Join(", ", this.argumentsExperssions) + "]";
            }
            return this.callable.ToString() + "(" + String.Join(", ", this.argumentsExperssions) + ")";
        }
    }
}