using System.Dynamic;

namespace Lumen.Lang {
	interface Debuggable : Value {
        DynamicMetaObject Inspect();
    }
}
