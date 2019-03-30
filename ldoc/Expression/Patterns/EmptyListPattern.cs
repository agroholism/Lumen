using System;
using System.Collections.Generic;

namespace ldoc {
    internal class EmptyListPattern : IPattern {
        public override String ToString() {
            return "[]";
        }
    }
}