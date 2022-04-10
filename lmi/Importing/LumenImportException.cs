using System;

using Lumen.Lang;

namespace Lumen.Lmi.Importing {
	internal class LumenImportException : LumenException {
		public LumenImportException(String message) : base(message) {
			
		}

		public LumenImportException(String message, Int32 line = -1, String fileName = null) : base(message, line, fileName) {

		}
	}
}
