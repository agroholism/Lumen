using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Light {
    internal class TypePattern : IPattern {
        private String id;
        private List<IPattern> subPatterns;

        public TypePattern(String id, List<IPattern> subPatterns) {
            this.id = id;
            this.subPatterns = subPatterns;
        }

        public Expression Closure(List<String> visible, Scope scope) {
            return new TypePattern(this.id, this.subPatterns.Select(i => i.Closure(visible, scope) as IPattern).ToList());
        }

        public Value Eval(Scope e) {
            throw new System.NotImplementedException();
        }

        public List<String> GetDeclaredVariables() {
            List<String> res = new List<String>();

            if (this.id.StartsWith("'")) {
                res.Add(this.id);
            }

            foreach (IPattern i in this.subPatterns) {
                res.AddRange(i.GetDeclaredVariables());
            }

            return res;
        }

        public System.Boolean Match(Value value, Scope scope) {
            Value needed;

            if (!this.id.StartsWith("'")) {
                needed = scope[this.id];
            } else {
                if (scope.ExistsInThisScope(this.id)) {
                    needed = scope[this.id];
                } else {
                    needed = value.Type;
                    scope[this.id] = value.Type;
                }
            }

            if (needed is Constructor ctor) {
                if (ctor.IsParentOf(value)) {

                    if(this.subPatterns.Count != ctor.Fields.Count) {
                        return false;
                    }

                    for (System.Int32 i = 0; i < ctor.Fields.Count; i++) {
                        if ((value as IObject).TryGetField(ctor.Fields[i], out Value fvalue)) {
                            if (!this.subPatterns[i].Match(fvalue, scope)) {
                                return false;
                            }
                        }
                    }

                    return true;
                }
                return false;
            }

            if (needed is SingletonConstructor) {
                return needed == value;
            }

            if (needed is Module m) {
                if (m.IsParentOf(value)) {
                    return this.subPatterns[0].Match(value, scope);
                }
            }
            return false;
        }

        public override String ToString() {
            return $"({this.id} {String.Join(" ", this.subPatterns)})";
        }
    }
}