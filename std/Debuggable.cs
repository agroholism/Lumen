using System;
using System.Collections.Generic;

namespace Lumen.Lang {
    interface Debuggable : Value {
        Dictionary<Text, Tuple<Value, Int32>> Inspect();
    }
}
