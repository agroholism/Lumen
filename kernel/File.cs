using System;

namespace Lumen.Lang.Std {
	public class File : Value {
		public System.IO.FileInfo Inner { get; internal set; }

		public File(System.IO.FileInfo value) {
			this.Inner = value;
		}

		public KType Type => StandartModule.File;

		public Value Clone() {
			throw new NotImplementedException();
		}

		public Int32 CompareTo(Object obj) {
			throw new NotImplementedException();
		}

		public Boolean ToBool(Scope e) {
			throw new NotImplementedException();
		}

		public Double ToDouble(Scope e) {
			throw new NotImplementedException();
		}

		public String ToString(Scope e) {
			return "<file:" + this.Inner.FullName + ">";
		}
	}
}
