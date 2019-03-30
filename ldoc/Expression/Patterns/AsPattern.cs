using System;
using System.Collections.Generic;

namespace ldoc {
    internal class AsPattern : IPattern {
        private IPattern listPattern;
        private String ide;

        public AsPattern(IPattern listPattern, String ide) {
            this.listPattern = listPattern;
            this.ide = ide;
        }

        public override String ToString() {
            return $"{this.listPattern.ToString()} as {this.ide}";
        }
    }
}