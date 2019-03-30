using System.Collections.Generic;

namespace ldoc {
    internal class GetIndexE : Expression {
        private Expression res;
        private List<Expression> indices;

        public GetIndexE(Expression res, List<Expression> indices) {
            this.res = res;
            this.indices = indices;
        }
    }
}