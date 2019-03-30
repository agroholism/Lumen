using System.Collections.Generic;
using System.Linq;
using System;

namespace ldoc {
    internal class ValuePattern : IPattern {
        private String value;

        public ValuePattern(String value) {
            this.value = value;
        }

        public override String ToString() {
            return this.value.ToString();
        }
    }

    internal class ListPattern : IPattern {
        private List<IPattern> patterns;

        public ListPattern(List<IPattern> patterns) {
            this.patterns = patterns;
        }

        public override String ToString() {
            return $"[{String.Join("; ", this.patterns)}]";
        }
    }
}