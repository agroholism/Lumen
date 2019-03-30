using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Light {
    internal class ContextPattern : IPattern {
        private System.String id;
        private String subPatterns;
        private IPattern pattern;

        public ContextPattern(System.String id, String subPatterns, IPattern pattern) {
            this.id = id;
            this.subPatterns = subPatterns;
            this.pattern = pattern;
        }

        public Expression Closure(List<System.String> visible, Scope scope) {
            return this;
        }

        public Value Eval(Scope e) {
            throw new System.NotImplementedException();
        }

        public List<String> GetDeclaredVariables() {
            List<String> res = new List<String>();

            if (this.subPatterns.StartsWith("'")) {
                res.Add(this.subPatterns);
            }

            res.AddRange(this.pattern.GetDeclaredVariables());
            
            return res;
        }

        public Boolean Match(Value value, Scope scope) {
            TypeClass tc = scope[this.id] as TypeClass;

            if (this.subPatterns.StartsWith("'")) {
                Module m = GetModule(value.Type);
                scope[this.subPatterns] = value.Type;
                if(!m.IsDerivedFrom(tc)) {
                    return false;
                }

                return this.pattern.Match(value, scope);
            }


            return false;
        }

        public Module GetModule(IObject obj) {
            if(obj is Module m) {
                return m;
            }

            return obj.Parent as Module;
        }

        public override String ToString() {
            return $"{this.id} {this.subPatterns} => {this.pattern}";
        }
    }
}