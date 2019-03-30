using System;
using System.Collections.Generic;
using System.Linq;

namespace ldoc {
    internal class TypeDeclarationE : Expression {
        public String name;
        public List<Expression> members;
        public Dictionary<String, List<String>> conStringuctors;

        public TypeDeclarationE(String name, Dictionary<String, List<String>> conStringuctors, List<Expression> members) {
            this.name = name;
            this.conStringuctors = conStringuctors;
            this.members = members;
        }
    }
}