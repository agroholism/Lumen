using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

using String = System.String;

namespace Lumen.Light {
    public class IdExpression : Expression {
        public String id;
        public Int32 line;
        public String file;

        public IdExpression(String id, Int32 line, String file) {
            this.id = id;
            this.line = line;
            this.file = file;
        }
        public Value Eval(Scope e) {
            if (!e.IsExsists(this.id)) {
                throw new LumenException($"unknown identifiter '{this.id}'") { file = this.file, line = this.line };
            }

            return e.Get(this.id);
        }

        public Expression Closure(List<String> visible, Scope thread) {
            if (!(visible.Contains(this.id))) {
                if (!thread.IsExsists(this.id)) {
                    throw new LumenException($"unknown identifiter '{this.id}'") { file = this.file, line = this.line };
                }
                return new ValueE(thread.Get(this.id));
            }

            return this;
        }

        public override String ToString() {
            return this.id;
        }
    }
}
