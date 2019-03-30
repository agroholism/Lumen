using System.Collections.Generic;

namespace ldoc {
    internal class VariablePattern : IPattern {
        private System.String id;

        public VariablePattern(System.String id) {
            this.id = id;
        }

        public override System.String ToString() {
            return this.id;
        }
    }
}