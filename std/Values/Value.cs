using System;

namespace Lumen.Lang {
    public interface Value : IComparable {
        IObject Type { get; }

        String ToString(Scope e);

        Value Clone();
    }
}
