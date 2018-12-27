namespace Lumen.Lang.Std {
	internal sealed class RVoid : Record {
		internal RVoid() {
			this.meta = new TypeMetadata {
				Name = "void"
			};
		}
	}
}
