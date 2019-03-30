using System.Collections.Generic;

namespace ldoc {
    internal class OrPattern : IPattern {
        private IPattern result;
        private IPattern second;

        public OrPattern(IPattern result, IPattern second) {
            this.result = result;
            this.second = second;
        }

        public override System.String ToString() {
            return $"{this.result} | {this.second}";
        }
    }
}