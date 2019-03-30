using System;
using System.Collections.Generic;

namespace ldoc {
    public interface IPattern : Expression {

    }

    public class NamePattern : IPattern {
        private String id;

        public NamePattern(String id) {
            this.id = id;
        }

        public override String ToString() {
            return this.id.ToString();
        }
    }
}
