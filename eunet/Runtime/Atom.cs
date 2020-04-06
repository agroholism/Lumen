using System;

namespace Argent.Xenon.Runtime {
	public struct Atom : ImmutObject, IEquatable<Atom> {
		public Double internalValue;

		public Atom(Double internalValue) {
			this.internalValue = internalValue;
		}

		public KsTypeable Type => XnStd.AtomType;

		public override Boolean Equals(Object obj) {
			if(obj is Atom atom) {
				return atom.internalValue == this.internalValue;
			}

			return base.Equals(obj);
		}

		public Boolean Equals(Atom other) {
			return this.internalValue == other.internalValue;
		}

		public override Int32 GetHashCode() {
			return 1989885529 + this.internalValue.GetHashCode();
		}

		public override String ToString() {
			return this.internalValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}

		public static Boolean operator ==(Atom left, Atom right) {
			return left.Equals(right);
		}

		public static Boolean operator !=(Atom left, Atom right) {
			return !(left == right);
		}
	}

	public struct Bool : XnObject, IEquatable<Bool> {
		public Boolean internalValue;

		public Bool(Boolean internalValue) {
			this.internalValue = internalValue;
		}

		public KsTypeable Type => XnStd.AtomType;

		public override Boolean Equals(Object obj) {
			if (obj is Bool b) {
				return b.internalValue == this.internalValue;
			}

			return base.Equals(obj);
		}

		public Boolean Equals(Bool other) {
			return this.internalValue == other.internalValue;
		}

		public override Int32 GetHashCode() {
			return 1989885529 + this.internalValue.GetHashCode();
		}

		public override String ToString() {
			return this.internalValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}

		public static Boolean operator ==(Bool left, Bool right) {
			return left.Equals(right);
		}

		public static Boolean operator !=(Bool left, Bool right) {
			return !(left == right);
		}
	}
}
