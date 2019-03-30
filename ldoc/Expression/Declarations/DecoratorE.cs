using System;
using System.Collections.Generic;

namespace ldoc {
    internal class AttributeExpression : Expression {
        internal List<Expression> attributes;
        internal Expression declaration;

        public AttributeExpression(List<Expression> attributes, Expression declaration) {
            this.attributes = attributes;
            this.declaration = declaration;
        }
    }
}