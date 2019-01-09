using System;

namespace Lumen.Studio {
	public interface IRunResult {
		Boolean Success { get; }

		Int32 ErrorLine { get; }
		Int32 ErrorCharEnd { get; }
		Int32 ErrorCharBegin { get; }

		String ErrorFile { get; }
		String ErrorType { get; }
		String ErrorMessage { get; }
	}
}
