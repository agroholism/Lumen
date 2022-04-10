using System.Collections.Generic;
using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	/// <summary> Contains more important Kernel constants. </summary>
	public static class Const {
		/// <summary> const null </summary>
		public static IValue UNIT { get; } = new Unit();

		/// <summary> const true </summary>
		public static Logical TRUE { get; } = new Logical(true);

		/// <summary> const false </summary>
		public static Logical FALSE { get; } = new Logical(false);

		public static List<IPattern> SelfOther { get; } = new List<IPattern> {
			new NamePattern("self"),
			new NamePattern("other")
		};

		public static List<IPattern> Self { get; } = new List<IPattern> {
			new NamePattern("self")
		};
	}
}
