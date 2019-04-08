using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using System.Linq;
using System;

namespace Lumen.Light {
    internal class ValuePattern : IPattern {
        private Value value;

        public ValuePattern(Value value) {
            this.value = value;
        }

        public Expression Closure(List<String> visible, Scope scope) {
            return this;
        }

        public Value Eval(Scope e) {
            throw new NotImplementedException();
        }

        public List<String> GetDeclaredVariables() {
            return new List<String>();
        }

        public Boolean Match(Value value, Scope scope) {
            return this.value.Equals(value);
        }

        public override String ToString() {
            return this.value.ToString();
        }
    }

    internal class ListPattern : IPattern {
        private List<IPattern> patterns;

        public ListPattern(List<IPattern> patterns) {
            this.patterns = patterns;
        }

        public Expression Closure(List<String> visible, Scope scope) {
            return new ListPattern(this.patterns.Select(i => i.Closure(visible, scope) as IPattern).ToList());
        }

        public Value Eval(Scope e) {
            throw new NotImplementedException();
        }

        public List<String> GetDeclaredVariables() {
            List<String> res = new List<String>();

            foreach(IPattern i in this.patterns) {
                res.AddRange(i.GetDeclaredVariables());
            }

            return res;
        }

        public Boolean Match(Value value, Scope scope) {
            if (value is List list) {
                if(LinkedList.Count(list.value) != this.patterns.Count) {
                    return false;
                }

                Int32 index = 0;
                foreach(Value i in list.value) {
                    if(!this.patterns[index].Match(i, scope)) {
                        return false;
                    }
                    index++;
                }
                return true;
            }

            return false;
        }

        public override String ToString() {
            return $"[{String.Join("; ", this.patterns)}]";
        }
    }
}