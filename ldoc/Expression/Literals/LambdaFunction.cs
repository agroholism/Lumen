using System;
using System.Collections.Generic;
using System.Linq;

namespace ldoc {
    public class AnonymeDefine : Expression {
        public Expression Body;
        public Expression def;
        public Expression returnedType;
        private readonly List<Expression> otherContacts;
 
        public AnonymeDefine(Expression Body) {
            this.Body = Body;
            this.returnedType = null;
        }
    }
}