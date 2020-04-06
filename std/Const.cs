using Lumen.Lang.Expressions;
using System.Collections.Generic;

namespace Lumen.Lang {
    /// <summary> Contains more important Kernel constants. </summary>
    public static class Const {
		/// <summary> const null </summary>
		public static Value UNIT { get; } = new Unit();

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
}
