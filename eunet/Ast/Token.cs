using System;

namespace Argent.Xenon.Ast {
	class Token {
		internal TokenType Type { get; set; }
		internal String Text { get; set; }
		internal Int32 Line { get; set; }

		public Token(TokenType type, String text = null) {
			this.Type = type;
			this.Text = text;
		}

		public Token(TokenType type, String text, Int32 line) : this(type, text) {
			this.Line = line;
		}

		public override String ToString() {
			return $"Token [Type: {this.Type}, Text: {this.Text}, Line: {this.Line}]";
		}
	}
}
