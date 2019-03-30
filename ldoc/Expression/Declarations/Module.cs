using System;
using System.Collections.Generic;
using System.Linq;

namespace ldoc {
    internal class ModuleDeclaration : Expression {
        internal String moduleName;
        internal List<Expression> moduleExpressions;

        public ModuleDeclaration(String moduleName, List<Expression> moduleExpressions) {
            this.moduleName = moduleName;
            this.moduleExpressions = moduleExpressions;
        }
    }
}