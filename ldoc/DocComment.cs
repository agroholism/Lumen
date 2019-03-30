namespace ldoc {
    internal class DocComment : Expression {
        public Expression Inner;
        public string doc;

        public DocComment(Expression inner, string doc) {
            this.Inner = inner;
            this.doc = doc;
        }
    }
}