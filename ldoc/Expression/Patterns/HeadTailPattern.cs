using System.Collections.Generic;
using System;

namespace ldoc {
    internal class HeadTailPattern : IPattern {
        private String xName;
        private String xsName;

        public HeadTailPattern(String xName, String xsName) {
            this.xName = xName;
            this.xsName = xsName;
        }

        public override String ToString() {
            return $"{this.xName}::{this.xsName}";
        }
    }
}