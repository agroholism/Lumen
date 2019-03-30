using System;
using System.Collections.Generic;

namespace Lumen.Studio {
    public interface IRunResult {
        Boolean Success { get; }

        List<IError> Errors { get; }
    }

    public interface IBuildResult {
        Boolean Success { get; }

        List<IError> Errors { get; }
    }

    public interface IError {
        Int32 ErrorLine { get; }
        Int32 ErrorCharEnd { get; }
        Int32 ErrorCharBegin { get; }

        String ErrorFile { get; }
        String ErrorType { get; }
        String ErrorMessage { get; }
    }
}
