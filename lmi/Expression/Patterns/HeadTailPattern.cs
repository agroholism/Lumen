using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using System;

namespace Lumen.Light {
    internal class HeadTailPattern : IPattern {
        private String xName;
        private String xsName;

        public HeadTailPattern(String xName, String xsName) {
            this.xName = xName;
            this.xsName = xsName;
        }

        public Expression Closure(List<String> visible, Scope scope) {
            visible.AddRange(new List<String> { this.xName, this.xsName });

            return this;
        }

        public Value Eval(Scope e) {
            throw new NotImplementedException();
        }

        public List<String> GetDeclaredVariables() {
            return new List<String> { this.xName, this.xsName };
        }

        public Boolean Match(Value value, Scope scope) {
            if (value is List list) {
                if (LinkedList.IsEmpty(list.value)) {
                    return false;
                }

                scope.Bind(this.xName, list.value.Head);
                scope.Bind(this.xsName, new List(list.value.Tail));
                return true;
            }

            return false;
        }

        public override String ToString() {
            return $"{this.xName}::{this.xsName}";
        }
    }
}