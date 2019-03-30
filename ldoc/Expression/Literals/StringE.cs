using System;

namespace ldoc {
    internal class StringE : Expression {
        internal String text;

        public StringE(String text) {
            this.text = text;
        }
    }
}