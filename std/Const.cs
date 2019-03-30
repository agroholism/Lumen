using Lumen.Lang.Expressions;
using System.Collections.Generic;

namespace Lumen.Lang {
    /// <summary> Contains more important Kernel constants. </summary>
    public static class Const {
        /// <summary> const null </summary>
        public static Void UNIT { get; } = new Void();

        /// <summary> const true </summary>
        public static Bool TRUE { get; } = new Bool(true);

        /// <summary> const false </summary>
        public static Bool FALSE { get; } = new Bool(false);

        public static List<IPattern> ThisOther { get; } = new List<IPattern> {
                new NamePattern("this"),
                new NamePattern("other")
            };

        public static List<IPattern> This { get; } = new List<IPattern> {
                new NamePattern("this")
            };
    }

    [System.Flags]
    public enum EntityAttribute {
        PROPERTY = 1,
        FUNCTION = 2,
        CONSTRUCTOR = 4,
        MODULE = 8,
        CLASSTYPE = 16
    }
}
