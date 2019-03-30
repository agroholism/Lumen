using System;
using System.Collections.Generic;
using System.Linq;

namespace ldoc {
    internal class TypePattern : IPattern {
        private String id;
        private List<IPattern> subPatterns;

        public TypePattern(String id, List<IPattern> subPatterns) {
            this.id = id;
            this.subPatterns = subPatterns;
        }

        public override String ToString() {
            return $"({this.id} {String.Join(" ", this.subPatterns)})";
        }
    }
}