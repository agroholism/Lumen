using System;
using System.Collections.Generic;
using System.Linq;

namespace ldoc {
    public class FunctionDeclaration : Expression {
        public String NameFunction;
        public List<IPattern> arguments;
        public Expression Body;
        public Int32 line;
        public String file;

        public FunctionDeclaration(String NameFunction, List<IPattern> Args, Expression Body) {
            this.NameFunction = NameFunction;
            this.arguments = Args;
            this.Body = Body;
        }

        public FunctionDeclaration(String name, List<IPattern> arguments, Expression body, Int32 line, String file) : this(name, arguments, body) {
            this.line = line;
            this.file = file;
        }
    }
}