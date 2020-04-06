using System;
using System.Collections.Generic;

namespace Argent.Xenon.Runtime {
	public class Text : XnObject, IEquatable<Text> {
		public String internalValue;

		public Text(String internalValue) {
			this.internalValue = internalValue;
		}

		public KsTypeable Type => XnStd.TextType;

		public override Boolean Equals(Object obj) {
			if(obj is Text text) {
				return this.internalValue == text.internalValue;
			}

			return base.Equals(obj);
		}

		public Boolean Equals(Text other) {
			return other != null &&
				   this.internalValue == other.internalValue;
		}

		public override Int32 GetHashCode() {
			return 1989885529 + EqualityComparer<String>.Default.GetHashCode(this.internalValue);
		}

		public override String ToString() {
			return this.internalValue;
		}

		public static Boolean operator ==(Text left, Text right) {
			return EqualityComparer<Text>.Default.Equals(left, right);
		}

		public static Boolean operator !=(Text left, Text right) {
			return !(left == right);
		}
	}
}
